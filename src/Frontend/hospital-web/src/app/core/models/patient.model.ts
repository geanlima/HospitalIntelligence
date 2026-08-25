export interface Patient {
  id: string;
  name: string;
  birthDate: string;
  gender: string;
  sourceSystem?: string | null;
  externalId?: string | null;
}