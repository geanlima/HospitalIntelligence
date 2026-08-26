import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Admission } from '../models/admission.model';

@Injectable({
  providedIn: 'root'
})
export class AdmissionService {
  private readonly http = inject(HttpClient);

  getAdmissions(
    status?: string,
    unit?: string
  ): Observable<Admission[]> {
    let params = new HttpParams();

    if (status?.trim()) {
      params = params.set(
        'status',
        status.trim()
      );
    }

    if (unit?.trim()) {
      params = params.set(
        'unit',
        unit.trim()
      );
    }

    return this.http.get<Admission[]>(
      '/admissions',
      { params }
    );
  }
}
