import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { PatientFormComponent } from './pages/patient-form/patient-form.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'patient-form', component: PatientFormComponent },
];
