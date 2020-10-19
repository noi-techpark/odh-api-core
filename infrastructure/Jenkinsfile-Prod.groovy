pipeline {
    agent any

    environment {
        DOCKER_PROJECT_NAME = "odh-tourism-api"
        DOCKER_IMAGE = '755952719952.dkr.ecr.eu-west-1.amazonaws.com/odh-tourism-api'
        DOCKER_TAG = "prod-$BUILD_NUMBER"

		SERVER_PORT = "1011"
        
        PG_CONNECTION = credentials('odh-tourism-api-prod-pg-connection')
		MSS_USER = credentials('odh-tourism-api-test-mss-user')
		MSS_PSWD = credentials('odh-tourism-api-test-mss-pswd')
		LCS_USER = credentials('odh-tourism-api-test-lcs-user')
		LCS_PSWD = credentials('odh-tourism-api-test-lcs-pswd')
		MSS_MSGPSWD = credentials('odh-tourism-api-test-lcs-msgpswd')
		SIAG_USER = credentials('odh-tourism-api-test-siag-user')
		SIAG_PSWD = credentials('odh-tourism-api-test-siag-pswd')
		XMLDIR = credentials('odh-tourism-api-test-xmldir')
    }

    stages {
        stage('Configure') {
            steps {
                sh """
                    rm -f .env
                    cp .env.example .env
                    echo 'COMPOSE_PROJECT_NAME=${DOCKER_PROJECT_NAME}' >> .env
                    echo 'DOCKER_IMAGE=${DOCKER_IMAGE}' >> .env
                    echo 'DOCKER_TAG=${DOCKER_TAG}' >> .env

					echo 'SERVER_PORT=${SERVER_PORT}' >> .env
                    
                    echo 'PG_CONNECTION=${PG_CONNECTION}' >> .env
                """
            }
        }
        stage('Build') {
            steps {
                sh '''
                    aws ecr get-login --region eu-west-1 --no-include-email | bash
                    docker-compose --no-ansi -f docker-compose.yml build --pull
                    docker-compose --no-ansi -f docker-compose.yml push
                '''
            }
        }
        stage('Deploy') {
            steps {
               sshagent(['jenkins-ssh-key']) {
                    sh """
                        (cd infrastructure/ansible && ansible-galaxy install -f -r requirements.yml)
                        (cd infrastructure/ansible && ansible-playbook --limit=prod deploy.yml --extra-vars "release_name=${BUILD_NUMBER}")
                    """
                }
            }
        }
    }
}
