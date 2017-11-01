import axios, * as ax from 'axios';
import {Injectable, Optional} from '@angular/core';
import * as md from '../models/models';

// API implementation

@Injectable()
export class API {
  public BaseUrl: string;
  private http: ax.AxiosInstance;

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
  }

  // Lemma methods
  public getLemma(id: number): Promise<md.Lemmas> {
    return this.http.get('/lemmas/' + id)
      .then((data) => {
        return <md.Lemmas>data.data;
      });
  }

  public getLemmaWithData(id: number): Promise<md.Lemmas> {
    return this.http.get('/lemmas/extras/' + id)
      .then((data) => {
        return <md.Lemmas>data.data;
      });
  }

  // list methods
  public getList(id: number): Promise<md.Lists> {
    return this.http.get('/lists/' + id)
      .then((data) => {
        return <md.Lists>data.data.data;
      });
  }

  public async getSectionsInList(id: number): Promise<md.Sections[]> {
    const sections: md.Sections[] = await this.http.get(`/lists/${id}/sections`)
      .then((data) => {
        return <md.Sections[]> data.data.data;
      });
    const sectionPromises = [];
    for (let i = 0; i < sections.length; i++) {
      sectionPromises.push(this.http.get(`/sections/${sections[i].sectionId}/lemmas`)
        .then(async (data) => {
          const l: md.Lemmas[] = data.data.data;
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

  // Learning methods
  public async reviseLemma(id: number): Promise<boolean> {
    await this.http.post(`/learn/${id}/revise`).then(console.log);
    return true;
  }
}
