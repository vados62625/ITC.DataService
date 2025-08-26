FROM python:3.11-slim

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    gcc \
    g++ \
    python3-dev \
    libkrb5-dev \
    krb5-config \
    libssl-dev \
    libffi-dev \
    && rm -rf /var/lib/apt/lists/*

RUN pip install --no-cache-dir poetry

WORKDIR /app

COPY ITC.AnalyseService/pyproject.toml ./

RUN poetry config virtualenvs.create false && \
    poetry install --without dev --no-interaction --no-ansi

COPY ./ITC.AnalyseService .

ENV PYTHONPATH="/app/code:${PYTHONPATH}"

EXPOSE 8000

CMD ["python", "code/src/api/rest/app.py"]
