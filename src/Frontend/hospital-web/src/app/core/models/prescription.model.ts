export interface Prescription {
  id: string;
  patientId: string;
  patientName: string;
  description: string;
  prescribedAtUtc: string;
  status: string;
}
