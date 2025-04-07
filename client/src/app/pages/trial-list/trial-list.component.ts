import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; // For *ngIf and *ngFor
import { TrialService } from '../../core/services/trial.service';
import { ClinicalTrial, PagedResult } from '../../models/clinical-trial.model';

@Component({
  selector: 'app-trial-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './trial-list.component.html',
  styleUrls: ['./trial-list.component.scss']
})
export class TrialListComponent implements OnInit {
  trials: ClinicalTrial[] = [];
  currentPage: number = 1;
  pageSize: number = 10;
  totalRecords: number = 0;
  loading: boolean = false;
  errorMessage: string = '';

  constructor(private trialService: TrialService) {}

  ngOnInit(): void {
    this.loadPage(this.currentPage);
  }

  loadPage(page: number): void {
    this.loading = true;
    this.trialService.getTrialsPaged(page, this.pageSize).subscribe({
      next: (result: PagedResult) => {
        this.trials = result.trials;
        this.totalRecords = result.totalRecords;
        this.currentPage = result.pageNumber;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching paged trials', err);
        this.errorMessage = 'Error loading trials';
        this.loading = false;
      }
    });
  }

  nextPage(): void {
    if (this.currentPage * this.pageSize < this.totalRecords) {
      this.loadPage(this.currentPage + 1);
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.loadPage(this.currentPage - 1);
    }
  }

  get totalPages(): number {
    return Math.ceil(this.totalRecords / this.pageSize);
  }
}
