import {Component, Input, OnInit} from '@angular/core';
import * as md from '../../models/models';

@Component({
  selector: 'app-list-card',
  templateUrl: './list-card.component.html',
  styleUrls: ['./list-card.component.css']
})
export class ListCardComponent implements OnInit {
  @Input()
  list: md.List;

  constructor() {
  }

  ngOnInit() {
  }

}
