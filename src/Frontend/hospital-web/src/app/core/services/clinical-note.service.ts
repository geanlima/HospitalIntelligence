import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ClinicalNote } from '../models/clinical-note.model';

@Injectable({
  providedIn: 'root'
})
export class ClinicalNoteService {
  private readonly http = inject(HttpClient);

  getClinicalNotes(
    noteType?: string
  ): Observable<ClinicalNote[]> {
    let params = new HttpParams();

    if (noteType?.trim()) {
      params = params.set('noteType', noteType.trim());
    }

    return this.http.get<ClinicalNote[]>(
      '/clinical-notes',
      { params }
    );
  }
}
