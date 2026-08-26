export interface VitalSign {
  id: string;
  patientId: string;
  patientName: string;
  measuredAtUtc: string;
  temperature?: number | null;
  heartRate?: number | null;
  respiratoryRate?: number | null;
  systolicBloodPressure?: number | null;
  diastolicBloodPressure?: number | null;
  oxygenSaturation?: number | null;
}
