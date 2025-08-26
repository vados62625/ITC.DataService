import asyncio
from contextlib import asynccontextmanager

import uvicorn
from fastapi import FastAPI

from py_gpn_kafka import build_kafka_app
from src.structure import structure


@asynccontextmanager
async def lifespan(app_: FastAPI):
    kafka_app = build_kafka_app(structure.broker_structure.broker_config)
    asyncio.create_task(kafka_app.start())  # noqa
    yield


app = FastAPI(lifespan=lifespan)

if __name__ == "__main__":
    uvicorn.run(app)
