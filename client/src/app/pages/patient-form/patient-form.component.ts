import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { NavMenuComponent } from '../../shared/components/nav-menu/nav-menu.component';
import { TrialService } from '../../core/services/trial.service';

@Component({
  selector: 'app-patient-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatStepperModule,
    MatIconModule,
    MatProgressSpinnerModule,
    NavMenuComponent,
  ],
  templateUrl: './patient-form.component.html',
  styleUrls: ['./patient-form.component.scss'],
})
export class PatientFormComponent implements OnInit {
  patientForm!: FormGroup;
  loading = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private trialService: TrialService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.patientForm = this.fb.group({
      notes: ['', Validators.required],
      age: [null, [Validators.min(0), Validators.max(120)]],
      sex: [''],
      condition: [''],
      city: [''],
      state: [''],
      country: [''],
    });
  }

  onSubmit(): void {
    if (this.patientForm.valid) {
      this.loading = true;
      this.error = '';

      const formData = this.patientForm.value;

      this.trialService.matchTrialsWithAi(formData).subscribe({
        next: (matchedTrials) => {
          this.trialService.setMatchedTrials(matchedTrials);
          this.router.navigate(['/trial-list']);
        },
        error: () => {
          this.error = 'Error matching trials. Please try again.';
          this.loading = false;
        },
        complete: () => {
          this.loading = false;
        },
      });
    } else {
      this.patientForm.markAllAsTouched();
    }
  }
}
