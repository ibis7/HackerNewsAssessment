import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTable, MatTableModule } from '@angular/material/table';
import { catchError, debounceTime, distinctUntilChanged, of } from 'rxjs';
import { SearchRequest } from '../../models/search-request.model';
import { SearchResponse } from '../../models/search-response.model';
import { StoryModel } from '../../models/story.model';
import { HackerNewsService } from '../../services/hacker-news.service';

@Component({
  selector: 'app-hacker-news-stories.component',
  imports: [
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatInputModule,
    FormsModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './hacker-news-stories.component.html',
  styleUrl: './hacker-news-stories.component.scss',
})
export class HackerNewsStoriesComponent implements OnInit, AfterViewInit {
  constructor(private readonly hackerNewsService: HackerNewsService) {}

  @ViewChild(MatTable) table!: MatTable<StoryModel>;

  private defaultSearchResponse: SearchResponse = {
    stories: [],
    totalLength: 0,
  };

  public searchControl = new FormControl('');
  public response: SearchResponse = this.defaultSearchResponse;
  public pageSizeOptions: number[] = [10, 20, 50];
  public displayedColumns: string[] = ['title', 'url'];
  public request: SearchRequest = {
    pageSize: 20,
    pageNumber: 0,
  };
  public isLoading: boolean = true;

  ngOnInit(): void {
    this.resetRequest();
    this.searchData();
  }

  ngAfterViewInit(): void {
    this.subscribeToSearchInput();
  }

  private subscribeToSearchInput() {
    this.searchControl.valueChanges
      .pipe(debounceTime(1000), distinctUntilChanged())
      .subscribe((term) => {
        this.request.searchTerm = term ? term : undefined;
        this.searchData();
      });
  }

  private resetRequest() {
    this.request = {
      pageSize: this.pageSizeOptions[0],
      pageNumber: 0,
    };
  }

  private searchData() {
    this.isLoading = true;

    this.hackerNewsService
      .getFilteredNews(this.request)
      .pipe(
        catchError((error) => {
          console.error('Error loading news:', error);
          this.isLoading = false;
          return of(this.defaultSearchResponse);
        }),
      )
      .subscribe((response) => {
        this.response = response;
        this.table.renderRows();
        this.isLoading = false;
      });
  }

  public sortChange($event: Sort) {
    const isAsc = $event.direction === 'asc';
    const isDesc = $event.direction === 'desc';

    this.request.isSortingAscending = isAsc ? true : isDesc ? false : undefined;
    this.request.sortedBy =
      this.request.isSortingAscending != undefined ? $event.active : undefined;

    this.searchData();
  }

  public changePaging($event: PageEvent) {
    this.request.pageNumber = $event.pageIndex;
    this.request.pageSize = $event.pageSize;
    this.searchData();
  }

  public searchTextChanged($event: Event) {
    this.request.searchTerm = '';
    this.searchData();
  }
}
