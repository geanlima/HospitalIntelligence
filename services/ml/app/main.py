"""Hospital Intelligence — serviço ML de estudo (Fase 18).

Modelos heurísticos versionados. Em produção seriam artefatos
treinados (joblib/sklearn) com pipeline de feature store.
"""

from __future__ import annotations

from datetime import datetime, timezone
from typing import Any

from fastapi import FastAPI
from pydantic import BaseModel, Field

app = FastAPI(title="Hospital ML Service", version="1.0.0")


class FeatureVector(BaseModel):
    patient_id: str
    age_years: int = 0
    length_of_stay_days: int = 0
    active_alert_count: int = 0
    pending_exam_count: int = 0
    latest_spo2: float = 97.0
    latest_heart_rate: float = 80.0
    active_prescription_count: int = 0
    has_medical_note: bool = False


class Prediction(BaseModel):
    model_name: str
    model_version: str
    score: float
    label: str
    features: dict[str, float]
    predicted_at_utc: datetime


def _clamp(value: float) -> float:
    return max(0.0, min(1.0, value))


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}


@app.get("/models")
def models() -> list[dict[str, Any]]:
    return [
        {
            "name": "no-show",
            "version": "1.0.0",
            "algorithm": "logistic-heuristic",
            "metrics": {"auc": 0.71, "precision": 0.64, "recall": 0.58},
            "trained_at_utc": "2026-08-01T00:00:00Z",
            "drift_detected": False,
            "drift_notes": "PSI < 0.1",
        },
        {
            "name": "discharge",
            "version": "1.1.0",
            "algorithm": "logistic-heuristic",
            "metrics": {"auc": 0.76, "precision": 0.69, "recall": 0.66},
            "trained_at_utc": "2026-08-01T00:00:00Z",
            "drift_detected": False,
            "drift_notes": "Estável",
        },
        {
            "name": "deterioration",
            "version": "1.0.2",
            "algorithm": "logistic-heuristic",
            "metrics": {"auc": 0.80, "precision": 0.72, "recall": 0.70},
            "trained_at_utc": "2026-08-01T00:00:00Z",
            "drift_detected": False,
            "drift_notes": "Estável",
        },
    ]


@app.post("/predict/no-show", response_model=Prediction)
def predict_no_show(features: FeatureVector) -> Prediction:
    score = _clamp(
        0.15
        + (0.08 if features.age_years > 65 else 0)
        + features.active_alert_count * 0.05
        + features.pending_exam_count * 0.04
        - (0.06 if features.has_medical_note else 0)
    )
    return _build("no-show", "1.0.0", score, "high-risk" if score >= 0.45 else "low-risk", features)


@app.post("/predict/discharge", response_model=Prediction)
def predict_discharge(features: FeatureVector) -> Prediction:
    score = _clamp(
        0.25
        + (0.2 if features.length_of_stay_days >= 3 else 0.05)
        + (0.15 if features.has_medical_note else 0)
        - features.active_alert_count * 0.08
        - features.pending_exam_count * 0.07
        - (0.2 if features.latest_spo2 < 92 else 0)
    )
    return _build(
        "discharge",
        "1.1.0",
        score,
        "likely-discharge" if score >= 0.55 else "stay",
        features,
    )


@app.post("/predict/deterioration", response_model=Prediction)
def predict_deterioration(features: FeatureVector) -> Prediction:
    score = _clamp(
        0.1
        + (
            0.35
            if features.latest_spo2 < 92
            else 0.15
            if features.latest_spo2 < 95
            else 0
        )
        + (
            0.2
            if features.latest_heart_rate > 110
            else 0.15
            if features.latest_heart_rate < 50
            else 0
        )
        + features.active_alert_count * 0.1
    )
    return _build(
        "deterioration",
        "1.0.2",
        score,
        "elevated" if score >= 0.5 else "stable",
        features,
    )


def _build(
    name: str,
    version: str,
    score: float,
    label: str,
    features: FeatureVector,
) -> Prediction:
    return Prediction(
        model_name=name,
        model_version=version,
        score=round(score, 4),
        label=label,
        features={
            "age_years": float(features.age_years),
            "los_days": float(features.length_of_stay_days),
            "active_alerts": float(features.active_alert_count),
            "pending_exams": float(features.pending_exam_count),
            "spo2": float(features.latest_spo2),
            "heart_rate": float(features.latest_heart_rate),
            "active_rx": float(features.active_prescription_count),
            "has_medical_note": 1.0 if features.has_medical_note else 0.0,
        },
        predicted_at_utc=datetime.now(timezone.utc),
    )
