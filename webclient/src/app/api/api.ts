import axios, * as ax from 'axios';
import {Injectable, Optional} from '@angular/core';
import * as md from '../models/models';

// API implementation

@Injectable()
export class API {
  private http: ax.AxiosInstance;
  public BaseUrl: string;


  constructor(@Optional() baseUrl: string) {
    if (!baseUrl) {
      baseUrl = 'http://localhost:52424/api';

    }
    this.BaseUrl = baseUrl;
    // create axios instance
    this.http = axios.create({
      baseURL: this.BaseUrl
    });
    this.http.interceptors.request.use(function (config: ax.AxiosRequestConfig) {
      config.headers = {Authorization: `Bearer ${localStorage.getItem('access_token')}`};
      return config;
    }, function (error) {
      return Promise.reject(error);
    });
    this.http.interceptors.response.use(function (res: ax.AxiosResponse): ax.AxiosResponse | Promise<ax.AxiosResponse> {
      /*
      * error handling for all routes:
      * returns the data object if the response is successful, else returns a nice error with all the data provided
      * */
      // handle errors actually returned by the API
      const result = res.data as md.Result;
      if (result.success) {
        res.data = result.data;
        return res;
      } else {
        // i.e. server returned error
        const err = new Error(result.errorMessage);
        const promise = new Promise(function (resolve, reject) {
          reject(err);
        });
        return promise;
      }

    }, function (error) {
      return Promise.reject(error);
    });
  }

  // Lemma methods
  public getLemma(id: number): Promise<md.Lemma> {
    return this.http.get('/lemmas/' + id)
      .then((data) => {
        return <md.Lemma>data.data;
      });
  }

  public getLemmaWithData(id: number): Promise<md.Lemma> {
    return this.http.get('/lemmas/extras/' + id)
      .then((data) => {
        return <md.Lemma>data.data;
      });
  }

  // list methods
  public getList(id: number): Promise<md.List> {
    return this.http.get('/lists/' + id)
      .then((data) => {
        return <md.List>data.data;
      });
  }

  public createList(name: string, desc: string): Promise<md.List> {
    const data = {'name': name, 'description': desc};
    console.log(data);
    return this.http.post('/lists', data).then((res) => {
      return <md.List>res.data;
    });
  }

  public getMyLists(): Promise<md.List[]> {
    return this.http.get('/lists/mine').then(res => <md.List[]> res.data);
  }

  public async getSectionsInList(id: number): Promise<md.Section[]> {
    const sections: md.Section[] = await this.http.get(`/lists/${id}/sections`)
      .then((data) => {
        return <md.Section[]> data.data;
      });
    const sectionPromises = [];
    for (let i = 0; i < sections.length; i++) {
      sectionPromises.push(this.http.get(`/sections/${sections[i].sectionId}/lemmas`)
        .then(async (data) => {
          const l: md.Lemma[] = data.data;
          const promises = [];
          for (let j = 0; j < l.length; j++) {
            promises.push(this.getLemmaWithData(l[j].lemmaId));
          }
          await Promise.all(promises).then(values => {
            sections[i].sectionWords = values;
          });
          return sections[i];
        }));
    }
    return Promise.all(sectionPromises).then((values) => {
      return values;
    });
  }

  // section methods
  public async modifySectionLemmas(sectionId: number, idsToDelete: number[], idsToAdd: number[]): Promise<boolean> {
    const del = this.http.patch(`/sections/${sectionId}/lemmas`, {ids: idsToDelete});
    const add = this.http.post(`/sections/${sectionId}/lemmas`, {ids: idsToAdd});
    await Promise.all([del, add]);
    return true;
  }

  public getSection(sectionId: number): Promise<md.Section> {
    return this.http.get('/sections/' + sectionId).then(res => <md.Section> res.data);
  }

  public getLemmasInSection(sectionId: number): Promise<md.Lemma[]> {
    return this.http.get('/sections/' + sectionId + '/lemmas').then(res => <md.Lemma[]> res.data);
  }

  // Learning methods
  public async reviseLemma(id: number): Promise<boolean> {
    await this.http.post(`/learn/${id}/revise`).then(console.log);
    return true;
  }


  // searching

  public async searchForLemma(search: string): Promise<md.Lemma[]> {
    return this.http.get(`/lemmas/search?query=${search}`)
      .then((data) => {
        return <md.Lemma[]>data.data;
      });
  }


  public async searchForList(search: string): Promise<md.List[]> {
    return this.http.get(`/lists/search?query=${search}`)
      .then((data) => {
        return <md.List[]>data.data;
      });
  }
}
