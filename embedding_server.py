from fastapi import FastAPI
from pydantic import BaseModel
from sentence_transformers import SentenceTransformer
from typing import Dict, List
import uvicorn

app = FastAPI()

# Загружаем MiniLM модель один раз при запуске
model = SentenceTransformer("all-MiniLM-L6-v2")


class EmbedRequest(BaseModel):
    texts: List[str]


class EmbedResponse(BaseModel):
    embeddings: List[List[float]]


@app.post("/embed", response_model=EmbedResponse)
async def embed(req: EmbedRequest):
    vectors = model.encode(req.texts, normalize_embeddings=True)
    return {"embeddings": vectors.tolist()}


@app.post("/embed-many")
def embed_many(req: Dict[str, List[str]]):
    texts = req["texts"]
    embeddings = model.encode(texts, normalize_embeddings=True).tolist()
    return embeddings

@app.get("/health")
async def health():
    return {"status": "ok"}


if __name__ == "__main__":
    uvicorn.run("embedding_server:app", host="0.0.0.0", port=8000)
