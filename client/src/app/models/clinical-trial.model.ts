export interface ClinicalTrial {
  nctId: string;
  briefTitle: string;
  officialTitle?: string;
  briefSummary?: string;
  conditions?: string[];
  keywords?: string[];
  phases?: string[];
  overallStatus: string;
  startDate?: string;
  completionDate?: string;
  firstSubmittedDate?: string;
  firstPostDate?: string;
  lastUpdateSubmitDate?: string;
  lastUpdatePostDate?: string;
  studyType?: string;
  healthyVolunteers?: boolean;
  sex?: string;
  minimumAge?: string;
  maximumAge?: string;
  eligibilityCriteria?: string;
  locations?: {
    status?: string;
    city?: string;
    state?: string;
    country?: string;
  }[];
  interventions?: {
    name: string;
    type: string;
    description?: string;
  }[];
}

export interface Intervention {
  id: number;
  name?: string;
  type?: string;
  description?: string;
  clinicalTrialNctId: string;
}

export interface Location {
  id: number;
  status?: string;
  city?: string;
  state?: string;
  zip?: string;
  country?: string;
  clinicalTrialNctId: string;
}
