import { ComponentFixture, TestBed } from '@angular/core/testing';

import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { HackerNewsService } from '../../services/hacker-news.service';
import { HackerNewsStoriesComponent } from './hacker-news-stories.component';

describe('HackerNewsStoriesComponent', () => {
  let component: HackerNewsStoriesComponent;
  let fixture: ComponentFixture<HackerNewsStoriesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HackerNewsStoriesComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        HackerNewsService,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(HackerNewsStoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
