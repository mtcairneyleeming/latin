import {Component, OnInit} from '@angular/core';
import {Lists} from '../../../models/Lists';
import {API} from '../../../api/api';

@Component({
  selector: 'app-find',
  templateUrl: './find.component.html',
  styleUrls: ['./find.component.css']
})
export class FindComponent implements OnInit {
  public lists: Lists[];

  constructor(private http: API) {
  }

  ngOnInit() {
  }

  async makeSearch(search: string) {
    this.lists = await this.http.searchForList(search);
  }

}
