import {Component, OnInit} from '@angular/core';
import {API} from '../../../api/api';
import {Router} from '@angular/router';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {

  constructor(private http: API,
              private router: Router) {
  }

  ngOnInit() {
  }

  createNew(name: string, desc: string) {
    this.http.createList(name, desc).then((val) => {
      this.redirectToListPage(val.listId);
    });
  }

  cloneList(name: string, desc: string, listID: number) {
    // TODO: actually do
  }

  redirectToListPage(listID: number) {
    this.router.navigate(['/lists', listID]);
  }
}

