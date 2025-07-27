from fastapi import FastAPI
from pydantic import BaseModel
from sentence_transformers import SentenceTransformer
from llama_cpp import Llama
from typing import Dict, List
import os

app = FastAPI()

# ───────────── Embedding ─────────────
embed_model = SentenceTransformer("all-MiniLM-L6-v2")
embed_model.encode(["warmup"], normalize_embeddings=True)

# ───────────── LLM (GGUF) ─────────────
llm = Llama(
    model_path="zephyr.gguf",  # rename as needed
    n_ctx=2048,
    n_threads=os.cpu_count(),
    n_gpu_layers=0,  # 0 = CPU only
    f16_kv=True,
    verbose=False
)

# ───────────── Models ─────────────
class EmbedRequest(BaseModel):
    texts: List[str]

class EmbedResponse(BaseModel):
    embeddings: List[List[float]]

class GenerateRequest(BaseModel):
    question: str
    context: str

class GenerateResponse(BaseModel):
    answer: str

# ───────────── Routes ─────────────

@app.post("/embed", response_model=EmbedResponse)
async def embed(req: EmbedRequest):
    vectors = embed_model.encode(req.texts, normalize_embeddings=True)
    return {"embeddings": vectors.tolist()}

@app.post("/embed-many")
async def embed_many(req: Dict[str, List[str]]):
    texts = req["texts"]
    vectors = embed_model.encode(texts, normalize_embeddings=True)
    return vectors.tolist()

# Prompt template
SYSTEM_PROMPT = """You are a Senior Staff Engineer from a top MAANG company.
Your job is to evaluate and guide engineers during mock technical interviews.

Always:
- Ask clarifying questions first
- Do not provide the full solution unless asked
- Help with hints and structure
- Keep your responses professional and technical

The user can ask anything about DSA, LeetCode, system design, or behavioral interviews.
"""

@app.post("/generate", response_model=GenerateResponse)
async def generate(req: GenerateRequest):
    full_prompt = f"""<|system|>\n{SYSTEM_PROMPT}\n<|user|>\n{req.question.strip()}\n\n{req.context.strip()}\n<|assistant|>\n"""

    output = llm(
        full_prompt,
        stop=["<|user|>", "<|system|>"],
        temperature=0.7,
        top_p=0.9,
        max_tokens=512,
    )

    return {"answer": output["choices"][0]["text"].strip()}

@app.get("/health")
async def health():
    return {"status": "ok"}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run("embedding_server:app", host="0.0.0.0", port=8000)
