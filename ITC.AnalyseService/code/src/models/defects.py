from pydantic import BaseModel


class Defects(BaseModel):
    defects: dict[str, float]
    file_id: str
