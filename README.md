# Recursive DNS Resolver

## Project Overview
This project implements a Recursive DNS Resolver that efficiently resolves domain names step by step using a caching mechanism. The resolver is designed to handle recursive queries while optimizing performance through caching and parallel execution of DNS resolution.

## Main Components

### Caching Mechanism
The resolver maintains a concurrent dictionary to store previously resolved domain names. This reduces redundant queries and speeds up future lookups. Before initiating a DNS request, the resolver checks if the domain is already cached and, if valid, returns the cached IP address.

### Recursive Resolution
The domain name is processed hierarchically by breaking it into segments. The resolution follows these steps:
1. Check the cache for a stored result.
2. If not found, determine the parent domain and resolve it recursively.
3. Query the next DNS server in the chain until the full domain name is resolved.
4. Store the resolved address in the cache for future use.

### Asynchronous Execution
The resolver leverages asynchronous programming with `async/await` to perform non-blocking network operations. Multiple domain queries can be executed concurrently, significantly improving performance.

### Reverse Lookup Validation
To ensure the integrity of cached results, the resolver performs a reverse DNS lookup on stored IP addresses. If the result does not match the expected domain name, the cache entry is removed, and a fresh resolution is performed.
