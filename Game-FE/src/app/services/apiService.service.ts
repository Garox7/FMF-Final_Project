import { HttpClient, HttpHandler, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ApiService<T> {
  ENDPOINT: string = 'http://localhost:5183/api';

  constructor(private http: HttpClient, private route: Router) {}

  get<T>(path: string, options?: any) {
    return this.request<T>('GET', this.ENDPOINT + path, undefined, options);
  }

  post<T>(path: string, body: any, options?: any) {
    return this.request<T>('POST', this.ENDPOINT + path, body, options);
  }

  put<T>(path: string, body: any, options?: any) {
    return this.request<T>('PUT', this.ENDPOINT + path, options);
  }

  delete<T>(path: string, options?: any) {
    return this.request<T>('DELETE', this.ENDPOINT + path, undefined, options);
  }

  request<T>(
    method: string,
    url: string,
    body?: any,
    options?: any,
    withCredentials?: boolean
  ) {
    method = method.toUpperCase();

    if (!options) {
      options = {};
    }

    if (!options?.params) {
      options.params = {};
    }

    if (!options?.withCredentials)
      options.withCredentials = withCredentials || null;

    if (options.withCredentials) {
      const token = localStorage.getItem('token');
      options.params.token = token || '';
    }

    let header = new HttpHeaders();
    header = header.set('Content-Type', 'application/json');

    options.observe = 'body';
    options.header = header;
    options.body = body;

    // TO BE FIXED WITH A PROXY
    options.withCredentials = false;

    return this.http.request<T>(method, url, options).pipe(
      catchError((error) => {
        if (error.status === 401) {
          localStorage.removeItem('token');
          this.route.navigate(['']);
        }
        return throwError(() => error);
      })
    ) as Observable<T>;
  }
}
