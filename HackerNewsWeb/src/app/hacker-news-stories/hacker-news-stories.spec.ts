import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HackerNewsStories } from './hacker-news-stories';

describe('HackerNewsStories', () => {
  let component: HackerNewsStories;
  let fixture: ComponentFixture<HackerNewsStories>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HackerNewsStories],
    }).compileComponents();

    fixture = TestBed.createComponent(HackerNewsStories);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
