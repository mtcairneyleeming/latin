import {Component, OnInit} from '@angular/core';
import {List} from '../../../models/List';
import {API} from '../../../api/api';

@Component({
  selector: 'app-find',
  templateUrl: './find.component.html',
  styleUrls: ['./find.component.css']
})
export class FindComponent implements OnInit {
  public lists: List[];

  constructor(private http: API) {
  }

  ngOnInit() {
  }

  async makeSearch(search: string) {
    this.lists = await this.http.searchForList(search);
  }

}
