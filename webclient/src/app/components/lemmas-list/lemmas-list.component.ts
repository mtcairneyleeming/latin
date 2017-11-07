import {Component, Input, OnInit} from '@angular/core';
import {Lemma} from '../../models/Lemma';

@Component({
  selector: 'app-lemmas-list',
  templateUrl: './lemmas-list.component.html',
  styleUrls: ['./lemmas-list.component.css']
})
export class LemmasListComponent implements OnInit {

  @Input()
  lemmas: Lemma[];

  constructor() {
  }

  ngOnInit() {
  }

}
