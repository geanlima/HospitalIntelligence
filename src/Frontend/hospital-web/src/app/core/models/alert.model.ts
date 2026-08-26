export interface Alert {
  id: string;
  patientId: string;
  patientName: string;
  type: string;
  severity: string;
  description: string;
  createdAtUtc: string;
  status: string;
}
