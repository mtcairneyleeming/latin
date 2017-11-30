import {Component, HostListener, OnInit} from '@angular/core';
import {ActivatedRoute, ParamMap} from '@angular/router';
import 'rxjs/add/operator/switchMap';
import {List} from '../../models/List';
import {Section} from '../../models/Section';
import {Lemma} from '../../models/Lemma';
import {API} from '../../api/api';

export enum TestStatus {
  Uncompleted = 0,
  Correct = 1,
  Wrong = 2,
  Fixed = 3,
}


export class Test {
  constructor(public Prompt: string, // large central prompt
              public Tags: string[], // pills to show - e.g. noun-2, conjunction
              public OriginalLemma: Lemma, // actual word - shows data after correct/incorrect
              public Answers: string[], // answers pre split & trimmed
              public Status: TestStatus,
              public Current: boolean // whether this is the one currently being tested upon
  ) {
  }
}

@Component({
  selector: 'app-learn',
  templateUrl: './learn.component.html',
  styleUrls: ['./learn.component.css']
})
export class LearnComponent implements OnInit {
  // Current status
  list: List; // bare struct - no extra data except name for title
  section: Section; // same - only section name & id
  tests: Test[] = []; // tests for display
  testStatus = TestStatus;
  // display helper
  statusDisplays = new Map<TestStatus, string>();

  constructor(private route: ActivatedRoute, private http: API) {
    this.statusDisplays.set(TestStatus.Correct, '✓');
    this.statusDisplays.set(TestStatus.Wrong, '☓');
    this.statusDisplays.set(TestStatus.Uncompleted, '?');
    this.statusDisplays.set(TestStatus.Fixed, '½');

  }

  ngOnInit() {
    this.route.paramMap
      .subscribe((params: ParamMap) => {
        const sectionId = parseInt(params.get('sectionID'), 10);
        const sectionPromise: Promise<Section> = this.http.getSection(sectionId);
        sectionPromise.then((s: Section) => {
          this.section = s;
          const listPromise: Promise<List> = this.http.getList(s.listId);
          listPromise.then((l: List) => {
            this.list = l;
          });
        });
        const lemmasPromise: Promise<Lemma[]> = this.http.getLemmasInSection(sectionId);
        lemmasPromise.then((l: Lemma[]) => {
          this.buildTests(l);
        });
      });
  }

  buildTests(lemmas: Lemma[]) {
    // assign test types (currently only 1) to words and form into the list
    for (const lemma of lemmas) {
      let prompt, answer: string;
      if (Math.random() > 0.5) {
        prompt = lemma.lemmaText;
        answer = lemma.lemmaShortDef;
      } else {
        prompt = lemma.lemmaShortDef;
        answer = lemma.lemmaText;
      }

      this.tests.push(new Test(
        prompt,
        ['word'],
        lemma,
        answer.split(/[,|;]/),
        TestStatus.Uncompleted,
        false
      ));
    }
  }

  checkTest(testIndex: number, answer: string) {
    console.log(testIndex, answer);
    if (this.checkTypedInput(answer, this.tests[testIndex].Answers)) {
      if (this.tests[testIndex].Status === TestStatus.Uncompleted) {
        this.tests[testIndex].Status = TestStatus.Correct;
      } else {
        this.tests[testIndex].Status = TestStatus.Fixed;
      }
    } else {
      this.tests[testIndex].Status = TestStatus.Wrong;
    }
  }

  checkTypedInput(userInput: string, answers: string[]): boolean {
    // simple at the moment, but could be improved upon
    const input = userInput.trim();
    for (const ans of answers) {
      // nice and simple - exact answer given
      if (input === ans.trim()) {
        return true;
      }
      if (input === this.stripBrackets(ans).trim()) {
        return true;
      }
    }
    return false;
  }

  markCurrent(i: number) {
    console.log(i);
    for (const test of this.tests) {
      test.Current = false;
    }
    this.tests[i].Current = true;
    const currentInput;
    console.log(currentInput);
    currentInput.focus();
  }

  getCurrent(): number {
    for (let i = 0; i < this.tests.length; i++) {
      if (this.tests[i].Current === true) {
        return i;
      }
    }
    return -1;
  }

  moveCurrent(change: number) {
    let current = this.getCurrent();
    current += change;
    if (current < 0) {
      current = 0;
    } else if (current >= this.tests.length) {
      current = this.tests.length - 1;
    }
    this.markCurrent(current);
  }

  stripBrackets(input: string): string {
    // TODO: implement
    return input;
  }

  setBadgeClasses(i: number) {
    return {
      'badge-successs': this.tests[i].Status === TestStatus.Correct,
      'badge-danger': this.tests[i].Status === TestStatus.Wrong,
      'badge-primary': this.tests[i].Status === TestStatus.Fixed,
      'badge-secondary': this.tests[i].Status === TestStatus.Uncompleted,
    };
  }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    switch (event.key) {
      case 'ArrowUp':
        this.moveCurrent(-1);
        break;
      case 'ArrowDown':
        this.moveCurrent(1);
        break;
      case 'w':
        this.moveCurrent(-1);
        break;
      case 's':
        this.moveCurrent(1);
        break;
    }
  }
}
