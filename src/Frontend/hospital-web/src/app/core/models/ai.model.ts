export interface AiSource {
  sourceId: string;
  title: string;
  excerpt: string;
  score: number;
}

export interface IndexPatientClinicalRecordsResponse {
  patientId: string;
  indexedCount: number;
  indexedAtUtc: string;
}

export interface SearchClinicalKnowledgeRequest {
  query: string;
  patientId: string;
  topK?: number;
}

export interface SearchClinicalKnowledgeResponse {
  patientId: string;
  query: string;
  hits: AiSource[];
}

export interface AskAiRequest {
  question: string;
  patientId: string;
  promptKey?: string;
}

export interface AskAiResponse {
  answer: string;
  promptKey: string;
  provider: string;
  sources: AiSource[];
  interactionId: string;
  occurredAtUtc: string;
}

export interface ChartAuditFinding {
  code: string;
  category: string;
  severity: string;
  title: string;
  message: string;
  relatedSourceIds: string[];
}

export interface AuditPatientChartResponse {
  patientId: string;
  auditedAtUtc: string;
  overallRisk: string;
  summary: string;
  missingDocumentationCount: number;
  divergenceCount: number;
  financialGlosaRiskCount: number;
  findings: ChartAuditFinding[];
}

export interface ClinicalSafetyFinding {
  code: string;
  category: string;
  severity: string;
  title: string;
  message: string;
  relatedSourceIds: string[];
}

export interface AssessClinicalSafetyResponse {
  patientId: string;
  assessedAtUtc: string;
  overallRisk: string;
  summary: string;
  dischargeReady: boolean;
  dischargeBlockerCount: number;
  deteriorationScore: number;
  deteriorationBand: string;
  triageRecommendation: string;
  medicationIssueCount: number;
  findings: ClinicalSafetyFinding[];
}

export interface StructureVoiceNoteRequest {
  transcript: string;
  patientId?: string | null;
  noteType?: string;
}

export interface StructureVoiceNoteResponse {
  draftTitle: string;
  structuredContent: string;
  noteType: string;
  provider: string;
  patientId?: string | null;
}
