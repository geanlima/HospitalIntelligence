import { inject, Injectable } from '@angular/core';
import {
  HttpClient,
  HttpParams
} from '@angular/common/http';
import { Observable } from 'rxjs';

import { Patient } from '../models/patient.model';
import { Patient360 } from '../models/patient-360.model';

@Injectable({
  providedIn: 'root'
})
export class PatientService {
  private readonly http = inject(HttpClient);

  getPatients(
    name?: string,
    sourceSystem?: string
  ): Observable<Patient[]> {
    let params = new HttpParams();

    if (name?.trim()) {
      params = params.set(
        'name',
        name.trim()
      );
    }

    if (sourceSystem?.trim()) {
      params = params.set(
        'sourceSystem',
        sourceSystem.trim()
      );
    }

    return this.http.get<Patient[]>(
      '/patients',
      {
        params
      }
    );
  }

  getPatient360(
    patientId: string
  ): Observable<Patient360> {
    return this.http.get<Patient360>(
      `/patients/${patientId}/360`
    );
  }
}
