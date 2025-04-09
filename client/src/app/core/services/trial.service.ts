import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ClinicalTrial, PagedResult } from '../../models/clinical-trial.model';
import { PatientData } from '../../models/patient-data.model';

@Injectable({
  providedIn: 'root',
})
export class TrialService {
  private baseUrl = 'http://localhost:5230/api/trials';

  constructor(private http: HttpClient) {}

  matchTrialsWithAi(input: PatientData): Observable<ClinicalTrial[]> {
    return this.http.post<ClinicalTrial[]>(`${this.baseUrl}/match`, input);
  }

  // Call the paged endpoint
  getTrialsPaged(
    pageNumber: number,
    pageSize: number
  ): Observable<PagedResult> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResult>(`${this.baseUrl}/paged`, { params });
  }
}
