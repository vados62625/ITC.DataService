from pydantic import BaseModel, Field


class MSGError(BaseModel):
    error_msg: str = Field(default="Something went wrong", description="error message")
