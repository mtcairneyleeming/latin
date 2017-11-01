import {async, ComponentFixture, TestBed} from '@angular/core/testing';

import {LemmasListComponent} from './lemmas-list.component';

describe('LemmasListComponent', () => {
  let component: LemmasListComponent;
  let fixture: ComponentFixture<LemmasListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [LemmasListComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LemmasListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
