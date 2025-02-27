import { provideApollo } from 'apollo-angular';
import { HttpLink } from 'apollo-angular/http';
import { inject, NgModule } from '@angular/core';
import { ApolloClientOptions, InMemoryCache } from '@apollo/client/core';

/**
 * Creates and configures Apollo GraphQL component.
 */ 
export function createApollo(): ApolloClientOptions<any> {
  const uri = 'api/graphql';
  const httpLink = inject(HttpLink);

  return {
    link: httpLink.create({ uri }),
    cache: new InMemoryCache(),
    defaultOptions: {
      watchQuery: { fetchPolicy: 'no-cache' },
      query: { fetchPolicy: 'no-cache' }
    }
  };
}

@NgModule({
  providers: [provideApollo(createApollo)],
})
export class GraphQLModule {}
