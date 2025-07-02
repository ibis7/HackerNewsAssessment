import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTable, MatTableModule } from '@angular/material/table';
import { SearchRequest } from '../../models/search-request.model';
import { StoryModel } from '../../models/story.model';
import { HackerNewsService } from '../../services/hacker-news.service';

@Component({
  selector: 'app-hacker-news-stories.component',
  imports: [MatTableModule, MatPaginatorModule, MatSortModule],
  templateUrl: './hacker-news-stories.component.html',
  styleUrl: './hacker-news-stories.component.scss',
})
export class HackerNewsStoriesComponent implements OnInit {
  constructor(private readonly hackerNewsService: HackerNewsService) {}

  @ViewChild(MatTable) table!: MatTable<StoryModel>;

  public stories: StoryModel[] = [];
  public pageSizeOptions: number[] = [10, 20, 50];
  public displayedColumns: string[] = ['title', 'url'];
  public request: SearchRequest = {
    pageSize: 20,
    pageNumber: 0,
  };

  ngOnInit(): void {
    this.resetRequest();
    this.reloadData();
  }

  private resetRequest() {
    this.request = {
      pageSize: this.pageSizeOptions[0],
      pageNumber: 0,
    };
  }

  private reloadData() {
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

    this.reloadData();
  }

  public changePaging($event: PageEvent) {
    this.request.pageNumber = $event.pageIndex;
    this.request.pageSize = $event.pageSize;
    this.reloadData();
  }
}
