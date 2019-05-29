pipeline {
    agent any

    stages {
        stage('Test & Build') {
            steps {
                sh "docker-compose build"
            }
        }
    }
    post { 
        always { 
            sh 'docker-compose down || true'
        }
    }
}
