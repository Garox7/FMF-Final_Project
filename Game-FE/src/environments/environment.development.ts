export const environment = {
  production: true,
  apiAuthEndpoint:
    document.location.protocol + //http
    '//' +
    document.location.hostname + //indirizzo ip corrente
    ':' +
    5183, //la porta che dipende dal server di c#
};
