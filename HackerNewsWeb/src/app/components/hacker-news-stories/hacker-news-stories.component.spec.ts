import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HackerNewsStoriesComponent } from './hacker-news-stories.component';

describe('HackerNewsStoriesComponent', () => {
  let component: HackerNewsStoriesComponent;
  let fixture: ComponentFixture<HackerNewsStoriesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HackerNewsStoriesComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(HackerNewsStoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
