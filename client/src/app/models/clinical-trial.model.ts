export interface ClinicalTrial {
  nctId: string;
  briefTitle?: string;
  overallStatus?: string;
  briefSummary?: string;
  conditions?: string[];
  keywords?: string[];
  studyType?: string;
  eligibilityCriteria?: string;
  healthyVolunteers?: string;
  sex?: string;
  genderBased?: string;
  genderDescription?: string;
  minimumAge?: string;
  maximumAge?: string;
  startDate?: string;
  studyFirstSubmitDate?: string;
  studyFirstPostDate?: string;
  lastUpdateSubmitDate?: string;
  lastUpdatePostDate?: string;
  phases?: string;
  interventions?: Intervention[];
  locations?: Location[];
}

export interface Intervention {
  id?: number;
  name?: string;
  type?: string;
  description?: string;
}

export interface Location {
  id?: number;
  status?: string;
  city?: string;
  state?: string;
  zip?: string;
  country?: string;
}

export interface PagedResult {
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  trials: ClinicalTrial[];
}
