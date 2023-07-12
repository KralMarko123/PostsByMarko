cd ..
docker compose down -v
docker rm -f $(docker ps -a -q)
docker volume rm $(docker volume ls -q)
docker network rm $(docker network ls -q)
cd ./scripts