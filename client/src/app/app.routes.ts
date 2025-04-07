import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { MatchFormComponent } from './pages/match-form/match-form.component';
import { ResultsComponent } from './pages/results/results.component';
import { TrialListComponent } from './pages/trial-list/trial-list.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'match', component: MatchFormComponent },
  { path: 'results', component: ResultsComponent },
  {path: 'trials', component: TrialListComponent}
];
