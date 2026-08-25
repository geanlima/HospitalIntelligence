export interface Patient360 {
  patientId: string;
  name: string;
  birthDate: string;
  gender: string;
  sourceSystem?: string | null;
  externalId?: string | null;

  admissions: AdmissionSummary[];
  exams: ExamSummary[];
  prescriptions: PrescriptionSummary[];
  vitalSigns: VitalSignSummary[];
  clinicalNotes: ClinicalNoteSummary[];
  alerts: PatientAlert[];
  timeline: PatientTimelineItem[];
}

export interface AdmissionSummary {
  id: string;
  admissionDate: string;
  dischargeDate?: string | null;
  unit?: string | null;
  bed?: string | null;
  status: string;
}

export interface ExamSummary {
  id: string;
  name: string;
  requestedAtUtc: string;
  resultedAtUtc?: string | null;
  status: string;
}

export interface PrescriptionSummary {
  id: string;
  description: string;
  prescribedAtUtc: string;
  status: string;
}

export interface VitalSignSummary {
  id: string;
  measuredAtUtc: string;
  temperature?: number | null;
  heartRate?: number | null;
  respiratoryRate?: number | null;
  systolicBloodPressure?: number | null;
  diastolicBloodPressure?: number | null;
  oxygenSaturation?: number | null;
}

export interface ClinicalNoteSummary {
  id: string;
  createdAtUtc: string;
  professional: string;
  noteType: string;
  summary: string;
}

export interface PatientAlert {
  id: string;
  type: string;
  severity: string;
  description: string;
  createdAtUtc: string;
}

export interface PatientTimelineItem {
  id: string;
  occurredAtUtc: string;
  type: string;
  title: string;
  description: string;
}