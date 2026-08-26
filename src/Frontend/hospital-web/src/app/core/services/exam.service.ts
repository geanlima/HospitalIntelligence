import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Exam } from '../models/exam.model';

@Injectable({
  providedIn: 'root'
})
export class ExamService {
  private readonly http = inject(HttpClient);

  getExams(
    status?: string,
    name?: string
  ): Observable<Exam[]> {
    let params = new HttpParams();

    if (status?.trim()) {
      params = params.set('status', status.trim());
    }

    if (name?.trim()) {
      params = params.set('name', name.trim());
    }

    return this.http.get<Exam[]>('/exams', { params });
  }
}
