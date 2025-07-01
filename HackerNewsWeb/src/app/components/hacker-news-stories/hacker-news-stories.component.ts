import { Component, OnInit } from '@angular/core';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { StoryModel } from '../../models/story.model';

@Component({
  selector: 'app-hacker-news-stories.component',
  imports: [MatTableModule, MatPaginatorModule],
  templateUrl: './hacker-news-stories.component.html',
  styleUrl: './hacker-news-stories.component.scss',
})
export class HackerNewsStoriesComponent implements OnInit {
  public myDataArray: StoryModel[] = [
    { id: 1 },
    { id: 2 },
    { id: 3 },
    { id: 4 },
    { id: 5 },
    { id: 6 },
    { id: 7 },
    { id: 8 },
    { id: 9 },
  ];
  public displayedColumns: string[] = ['title', 'url'];

  ngOnInit(): void {
    this.myDataArray = [
      { id: 1 },
      { id: 2 },
      { id: 3 },
      { id: 4 },
      { id: 5 },
      { id: 6 },
      { id: 7 },
      { id: 8 },
      { id: 9 },
    ];
  }
}
