import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  AskAiRequest,
  AskAiResponse,
  AssessClinicalSafetyResponse,
  AuditPatientChartResponse,
  IndexPatientClinicalRecordsResponse,
  SearchClinicalKnowledgeRequest,
  SearchClinicalKnowledgeResponse,
  StructureVoiceNoteRequest,
  StructureVoiceNoteResponse
} from '../models/ai.model';

@Injectable({
  providedIn: 'root'
})
export class AiService {
  private readonly http = inject(HttpClient);

  indexPatient(
    patientId: string
  ): Observable<IndexPatientClinicalRecordsResponse> {
    return this.http.post<IndexPatientClinicalRecordsResponse>(
      `/ai/index/patients/${patientId}`,
      {}
    );
  }

  search(
    request: SearchClinicalKnowledgeRequest
  ): Observable<SearchClinicalKnowledgeResponse> {
    return this.http.post<SearchClinicalKnowledgeResponse>(
      '/ai/search',
      request
    );
  }

  ask(
    request: AskAiRequest
  ): Observable<AskAiResponse> {
    return this.http.post<AskAiResponse>(
      '/ai/ask',
      {
        question: request.question,
        patientId: request.patientId,
        promptKey: request.promptKey ?? 'clinical-chart-qa'
      }
    );
  }

  auditPatient(
    patientId: string
  ): Observable<AuditPatientChartResponse> {
    return this.http.post<AuditPatientChartResponse>(
      `/ai/audit/patients/${patientId}`,
      {}
    );
  }

  assessClinicalSafety(
    patientId: string
  ): Observable<AssessClinicalSafetyResponse> {
    return this.http.post<AssessClinicalSafetyResponse>(
      `/ai/clinical-safety/patients/${patientId}`,
      {}
    );
  }

  structureVoiceNote(
    request: StructureVoiceNoteRequest
  ): Observable<StructureVoiceNoteResponse> {
    return this.http.post<StructureVoiceNoteResponse>(
      '/ai/clinical-safety/voice-note',
      request
    );
  }
}
