from pydantic import BaseModel


class InputData(BaseModel):
    data: list[list[float]]
    FileId: str | None
