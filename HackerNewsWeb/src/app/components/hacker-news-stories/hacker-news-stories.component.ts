import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTable, MatTableModule } from '@angular/material/table';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { SearchRequest } from '../../models/search-request.model';
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
  ],
  templateUrl: './hacker-news-stories.component.html',
  styleUrl: './hacker-news-stories.component.scss',
})
export class HackerNewsStoriesComponent implements OnInit, AfterViewInit {
  constructor(private readonly hackerNewsService: HackerNewsService) {}

  @ViewChild(MatTable) table!: MatTable<StoryModel>;

  public searchControl = new FormControl('');
  public stories: StoryModel[] = [];
  public pageSizeOptions: number[] = [10, 20, 50];
  public displayedColumns: string[] = ['title', 'url'];
  public request: SearchRequest = {
    pageSize: 20,
    pageNumber: 0,
  };

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
    this.hackerNewsService
      .getFilteredNews(this.request)
      .subscribe((response) => {
        this.stories = response;
        this.table.renderRows();
      });
  }

  public sortChange($event: Sort) {
    const isAsc = $event.direction === 'asc';
    const isDesc = $event.direction === 'desc';

    this.request.isSortingAscending = isAsc ? true : isDesc ? false : undefined;
    this.request.sortedBy = isAsc || isDesc ? $event.active : undefined;

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
