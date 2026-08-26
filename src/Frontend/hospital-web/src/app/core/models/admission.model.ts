export interface Admission {
  id: string;
  patientId: string;
  patientName: string;
  admissionDate: string;
  dischargeDate?: string | null;
  unit?: string | null;
  bed?: string | null;
  status: string;
}