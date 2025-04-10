import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { PatientFormComponent } from './pages/patient-form/patient-form.component';
import { TrialListComponent } from './pages/trial-list/trial-list.component';
import { TrialDetailComponent } from './pages/trial-detail/trial-detail.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'patient-form', component: PatientFormComponent },
  { path: 'trial-list', component: TrialListComponent },
  { path: 'trial/:id', component: TrialDetailComponent },
  { path: '**', redirectTo: '' }
];
