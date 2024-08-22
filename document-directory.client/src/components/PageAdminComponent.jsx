import React, { Component } from 'react';
import { useEffect, useState } from 'react';
import { BrowserRouter, NavLink } from 'react-router-dom';
import { useNavigate, Navigate } from 'react-router-dom';
import { Divider, Menu, Switch } from 'antd';
import { UserOutlined, TeamOutlined } from '@ant-design/icons';

import ListUserComponent from './ListUserComponent';
import ListGroupComponent from './ListGroupComponent';


function PageAdminComponent() {
    const [isShowListUser, setIsShowListUser] = useState(false);
    const [isShowListGroup, setIsShowListGroup] = useState(false);
    

    const handleChange = (item) => {

        if (item.key === '1') {
            setIsShowListUser(true);
            setIsShowListGroup(false);
        }
        else {
            setIsShowListUser(false);
            setIsShowListGroup(true);
        }
    }

    

    const menuItems = [
        {
            key: '1',
            icon: <UserOutlined />,
            label: 'Все пользователи',
        },
        {
            key: '2',
            icon: <TeamOutlined />,
            label: 'Все группы',
        }
    ]

    return (
        <>
            <h3>Управление пользователями и группами</h3>
            <div style={{ display: 'flex' }}>
                <div>
                    <Menu id='menu'
                        style={{
                            width: 185,
                            textAlign: 'left',
                        }}
                        //defaultSelectedKeys={['1']}
                        onClick={handleChange}
                        items={menuItems}

                    />
                </div>
                <div>
                    {isShowListUser && <ListUserComponent />}
                    {isShowListGroup && <ListGroupComponent />}
                      
                </div>
            </div>
            
        </>
    )
}
export default PageAdminComponent;