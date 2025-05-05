path "auth/token/lookup-self" {
  capabilities = ["read"]
}

path "gateway/data/*" {
  capabilities = ["read", "list"]
}

path "gateway/metadata/*" {
  capabilities = ["list"]
}
