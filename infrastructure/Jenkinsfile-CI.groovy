pipeline {
    agent any

    environment {
        DOCKER_IMAGE = "app"
        DOCKER_TAG = "latest"
    }

    stages {
        stage('Configure') {
            steps {
                sh "rm -f .env"
                sh "cp .env.example .env"
                sh "echo 'DOCKER_IMAGE=${DOCKER_IMAGE}' >> .env"
                sh "echo 'DOCKER_TAG=${DOCKER_TAG}' >> .env"
            }
        }
        stage('Test') {
            steps {
                sh "docker-compose -f docker-compose.yml build"
            }
        }
    }
    post { 
        always { 
            sh 'docker-compose -f docker-compose.yml down || true'
        }
    }
}
