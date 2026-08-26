export interface ClinicalNote {
  id: string;
  patientId: string;
  patientName: string;
  professional: string;
  noteType: string;
  content: string;
  createdAtUtc: string;
}
