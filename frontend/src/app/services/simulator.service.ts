import { Injectable } from '@angular/core';
import { HttpClient, HttpParams  } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SimulatorService {

  private apiUrl = 'http://localhost:5280/api';

  constructor(private http: HttpClient) {}

  login(username: string, password: string) {
    return this.http.post(
      `${this.apiUrl}/auth/login`,
      {
        username,
        password
      },
      {
        responseType: 'text'
      }
    );
  }

  comment(comment: string) {
  return this.http.post(
    `${this.apiUrl}/comments`,
    {
      comment
    },
    {
      responseType: 'text'
    }
  );
}

  search(query: string) {
    const params = new HttpParams()
      .set('query', query);

    return this.http.get(
      `${this.apiUrl}/search`,
      {
        params,
        responseType: 'text'
      }
    );
  }

    testFilePath(path: string) {
    const params = new HttpParams()
      .set('path', path);

    return this.http.get(
      `${this.apiUrl}/files`,
      {
        params,
        responseType: 'text'
      }
    );
  }

    testCommand(command: string) {
    return this.http.post(
      `${this.apiUrl}/commands`,
      {
        command
      },
      {
        responseType: 'text'
      }
    );
  }


}