export interface Exam {
  id: string;
  patientId: string;
  patientName: string;
  name: string;
  requestedAtUtc: string;
  resultedAtUtc?: string | null;
  status: string;
  result?: string | null;
}
