import {Component, Input, OnInit} from '@angular/core';
import {Lemmas} from '../../models/models';

@Component({
  selector: 'app-lemmas-list',
  templateUrl: './lemmas-list.component.html',
  styleUrls: ['./lemmas-list.component.css']
})
export class LemmasListComponent implements OnInit {

  @Input()
  lemmas: Lemmas[];

  constructor() {
  }

  ngOnInit() {
  }

}
